package main
import "strconv"

import (
	"database/sql"
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"os"
	"strings"
	"time"
	

	"github.com/dgrijalva/jwt-go"
	_ "github.com/mattn/go-sqlite3"
)
// VERY VERY BAD IMPLEMENTATION OF GOLANG SERVER, THAT'S MY FIRST GOLANG SERVER DO NOT JUDGE ME PLEASE ;(


	type LoginRequest struct {
	Username string `json:"username"`
	Password string `json:"password"`
}
type VerifyTokenRequest struct {
	Token string `json:"Token"`
}
type LUser struct
{
	ID int `json: id`
	Username string `json: username`
	Password string `json: password`
	Coins string `json: coins`
}

// TokenClaims represents the claims in the JWT token
type TokenClaims struct {
	Username string `json:"username"`
	jwt.StandardClaims
}

var secretKey = []byte("your-secret-key")


func verifyTokenHandler(w http.ResponseWriter, r *http.Request) {
	log.Println("Client connected:", r.RemoteAddr)
	log.Println("Client requested: /verify")

	// Decode JSON from the request body
	var requestBody VerifyTokenRequest
    decoder := json.NewDecoder(r.Body)
    err := decoder.Decode(&requestBody)
    if err != nil {
        http.Error(w, "Failed to decode request body", http.StatusBadRequest)
        return
    }

	// var requestBody map[string]string
	// if err := json.NewDecoder(r.Body).Decode(&requestBody); err != nil {
	// 	log.Printf("Error decoding JSON request body: %v\n", err)
	// 	http.Error(w, fmt.Sprintf("Error decoding JSON request body: %v", err), http.StatusBadRequest)
	// 	return
	// }

	tokenString := requestBody.Token
	if tokenString == "" {
		log.Println("Missing or empty token in JSON body")
		http.Error(w, "Missing or empty token in JSON body", http.StatusBadRequest)
		return
	}

	valid, _, err := VerifyToken(tokenString)
	if err != nil {
		log.Printf("Error verifying token: %v\n", err)
		http.Error(w, fmt.Sprintf("Error verifying token: %v", err), http.StatusInternalServerError)
		return
	}

	if !valid {
		log.Println("Invalid token")
		http.Error(w, "Invalid token", http.StatusUnauthorized)
		return
	}

	log.Printf("Token verified successfully. Token: %s\n", tokenString)

	// Token is valid
	        // Respond with user information and token
			response := map[string]interface{}{
				"Success": true,
				"Message": "",
				"Token":   tokenString,
			}
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(response)
}


func generateToken(username string) (string, error) {
	log.Printf("Generating token for user: %s\n", username)

	expirationTime := time.Now().Add(24 * time.Hour) // Token expires in 24 hours
	claims := &TokenClaims{
		Username: username,
		StandardClaims: jwt.StandardClaims{
			ExpiresAt: expirationTime.Unix(),
		},
	}

	token := jwt.NewWithClaims(jwt.SigningMethodHS256, claims)

	tokenString, err := token.SignedString(secretKey)
	if err != nil {
		log.Printf("Error generating token for user %s: %v\n", username, err)
		return "", err
	}

	log.Printf("Token generated successfully for user %s\n", username)

	return tokenString, nil
}


func validateToken(tokenString string) (string, error) {
	log.Printf("Validating token: %s\n", tokenString)

	token, err := jwt.ParseWithClaims(tokenString, &TokenClaims{}, func(token *jwt.Token) (interface{}, error) {
		return secretKey, nil
	})

	if err != nil {
		log.Printf("Error validating token: %v\n", err)
		return "", err
	}

	claims, ok := token.Claims.(*TokenClaims)
	if !ok || !token.Valid {
		log.Println("Invalid token")
		return "", fmt.Errorf("Invalid token")
	}

	log.Printf("Token validated successfully. Username: %s\n", claims.Username)

	return claims.Username, nil
}

func VerifyToken(tokenString string) (bool, string, error) {
	log.Printf("Verifying token: %s\n", tokenString)

	// Check if "Bearer" is already present in the token
	if !strings.HasPrefix(tokenString, "Bearer ") {
		// If not, add "Bearer "
		tokenString = "Bearer " + tokenString
		log.Printf("Token formatted: %s\n", tokenString)
	}

	parts := strings.Split(tokenString, " ")
	if len(parts) != 2 || parts[0] != "Bearer" {
		log.Println("Invalid token format")
		return false, "", fmt.Errorf("Invalid token format")
	}

	token, err := jwt.ParseWithClaims(parts[1], &TokenClaims{}, func(token *jwt.Token) (interface{}, error) {
		return []byte("your-secret-key"), nil
	})

	if err != nil {
		log.Printf("Error parsing token: %v\n", err)
		return false, "", fmt.Errorf("Error parsing token: %v", err)
	}

	claims, ok := token.Claims.(*TokenClaims)
	if !ok || !token.Valid {
		log.Println("Invalid token")
		return false, "", fmt.Errorf("Invalid token")
	}

	log.Printf("Token verified successfully. Username: %s\n", claims.Username)

	return true, claims.Username, nil
}




func handleLoginRequest(w http.ResponseWriter, r *http.Request) {

    log.Println("Client connected:", r.RemoteAddr)
    log.Println("Client requested: /login")
	
    if r.Method != http.MethodPost {
        http.Error(w, "Invalid request method", http.StatusMethodNotAllowed)
        return
    }

    var loginRequest LoginRequest
    decoder := json.NewDecoder(r.Body)
    err := decoder.Decode(&loginRequest)
    if err != nil {
        http.Error(w, "Failed to decode request body", http.StatusBadRequest)
        return
    }

    // Open a connection to the SQLite database
    conn, err := sql.Open("sqlite3", "example.db")
    if err != nil {
        log.Println("Error opening SQLite database:", err)
        http.Error(w, "Internal Server Error", http.StatusInternalServerError)
        return
    }
    defer conn.Close()

    // Create a new prepared statement
    stmt, err := conn.Prepare("SELECT username, coins FROM users WHERE username=? AND password=?")
    if err != nil {
        log.Println("Error preparing SQL statement:", err)
        http.Error(w, "Internal Server Error", http.StatusInternalServerError)
        return
    }
    defer stmt.Close()

    // Execute the prepared statement with user input
    rows, err := stmt.Query(loginRequest.Username, loginRequest.Password)
    if err != nil {
        log.Println("Error executing query:", err)
        http.Error(w, "Internal Server Error", http.StatusInternalServerError)
        return
    }
    defer rows.Close()

    // Check if the user exists
    if rows.Next() {
        var user LUser
        // Scan user data
        err := rows.Scan(&user.Username, &user.Coins)
        if err != nil {
            log.Println("Error scanning user data:", err)
            http.Error(w, "Internal Server Error", http.StatusInternalServerError)
            return
        }

        // Generate a JWT token for the user
        token, err := generateToken(user.Username)
        if err != nil {
            log.Println("Error generating token:", err)
            http.Error(w, "Internal Server Error", http.StatusInternalServerError)
            return
        }

        // Respond with user information and token
        response := map[string]interface{}{
            "Success": true,
            "Message": "",
            "Token":   token,
        }
        w.Header().Set("Content-Type", "application/json")
        json.NewEncoder(w).Encode(response)
    } else {
        // User does not exist
		response := map[string]interface{}{
            "Success": false,
            "Message": "User does not exist in the database: " + loginRequest.Username,
        }
		w.Header().Set("Content-Type", "application/json")
        json.NewEncoder(w).Encode(response)
    }
}


func handleAttackRequest(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Invalid request method", http.StatusMethodNotAllowed)
		return
	}

	// Extract the token from the Authorization header
	tokenString := r.Header.Get("Authorization")
	if tokenString == "" {
		http.Error(w, "Unauthorized: Missing token", http.StatusUnauthorized)
		return
	}

	// Verify the token
	valid, username, err := VerifyToken(tokenString)
	if err != nil {
		log.Println("Error verifying token:", err)
		http.Error(w, "Internal Server Error", http.StatusInternalServerError)
		return
	}

	if !valid {
		log.Println("Invalid token:", tokenString)
		http.Error(w, "Unauthorized: Invalid token", http.StatusUnauthorized)
		return
	}
	// Open a connection to the SQLite database
	conn, err := sql.Open("sqlite3", "example.db")
	if err != nil {
		log.Println("Error opening SQLite database:", err)
		http.Error(w, "Internal Server Error", http.StatusInternalServerError)
		return
	}
	defer conn.Close()

	// Create a new prepared statement to update coins after the attack
	stmt, err := conn.Prepare("UPDATE users SET coins = coins - ? WHERE username = ?")
	if err != nil {
		log.Println("Error preparing SQL statement:", err)
		http.Error(w, "Internal Server Error", http.StatusInternalServerError)
		return
	}
	defer stmt.Close()

	// Execute the prepared statement with a fixed attack amount and the username
	_, err = stmt.Exec(50, username)
	if err != nil {
		log.Println("Error executing query:", err)
		http.Error(w, "Internal Server Error", http.StatusInternalServerError)
		return
	}

	// Select the updated coins after the attack
	var newCoins int
	err = conn.QueryRow("SELECT coins FROM users WHERE username = ?", username).Scan(&newCoins)
	if err != nil {
		log.Println("Error scanning user data:", err)
		http.Error(w, "Internal Server Error", http.StatusInternalServerError)
		return
	}

	// Respond with a success message and new coin value
	w.Write([]byte("Attack successful. Coins deducted: 50. New coins value: " + fmt.Sprint(newCoins)))
}

// UserRequest represents the JSON structure for user requests.
type UserRequest struct {
	Username string       `json:"Username"`
	Fields   []UserField  `json:"Fields"`
}

// UserField represents the JSON structure for individual user fields.
type UserField struct {
	OperationType string `json:"Operation"`
	Name          string `json:"Name"`
	Value         string `json:"Value"`
}

// UserResponse represents the JSON structure for the server response.
type UserResponse struct {
	Success  bool        `json:"Success"`
	Message  string      `json:"Message"`
	Username string      `json:"Username"`
	Fields   []UserField `json:"Fields"`
}

func handleGetData(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Invalid request method", http.StatusMethodNotAllowed)
		return
	}

	// Decode JSON from the request body
	var requestBody UserRequest
	decoder := json.NewDecoder(r.Body)
	err := decoder.Decode(&requestBody)
	if err != nil {
		http.Error(w, "Failed to decode request body", http.StatusBadRequest)
		return
	}


	valid, username, err := VerifyToken(requestBody.Username)
	if err != nil || !valid {
		// If the token is invalid, assume it might be a requestBody.Username and check if it exists in the database
		validUsername, err := CheckUsernameValidity(requestBody.Username)
		if err != nil || !validUsername {
			log.Println("Invalid token or username:", requestBody.Username)
			http.Error(w, "Unauthorized: Invalid token or username", http.StatusUnauthorized)
			return
		}
		// Use the provided string as the username
		username = requestBody.Username
	}

	// Extract user data update fields
	userFields := requestBody.Fields

	// Open a connection to the SQLite database
	conn, err := sql.Open("sqlite3", "example.db")
	if err != nil {
		log.Println("Error opening SQLite database:", err)
		http.Error(w, "Internal Server Error", http.StatusInternalServerError)
		return
	}
	defer conn.Close()

	// Response fields to accumulate the result
	var responseFields []UserField

	// Handle user data updates based on the received fields
	for _, field := range userFields {
		resultField, err := handleUserData(conn, username, field)
		if err != nil {
			log.Println("Error handling user data:", err)
			http.Error(w, "Internal Server Error", http.StatusInternalServerError)
			return
		}

		// Accumulate the result field
		responseFields = append(responseFields, resultField)
	}

	// Select all user data based on the username
	var user LUser
	err = conn.QueryRow("SELECT username, coins FROM users WHERE username = ?", username).
    Scan(&user.Username, &user.Coins)

	if err != nil {
		log.Println("Error scanning user data:", err)
		http.Error(w, "Internal Server Error", http.StatusInternalServerError)
		return
	}

	// Prepare the response JSON
	response := UserResponse{
		Success:  true,
		Message:  "User data retrieved successfully",
		Username: user.Username,
		Fields:   responseFields,
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(response)
}
func handleUserData(conn *sql.DB, username string, field UserField) (UserField, error) {
    log.Println("Operation type:", field.OperationType)
    switch field.OperationType {
    case "GET":
        // Handle GET operation
        var result string
        err := conn.QueryRow("SELECT "+field.Name+" FROM users WHERE username = ?", username).Scan(&result)
        if err != nil {
            return field, err
        }
        field.Value = result
        return field, nil
    case "SET":
        // Handle SET operation
        _, err := conn.Exec("UPDATE users SET "+field.Name+" = ? WHERE username = ?", field.Value, username)
        if err != nil {
            return field, err
        }
        return field, nil
    case "UPDATE":
        // Handle UPDATE operation
        // Add your logic for UPDATE operation here
        // Example: conn.Exec("UPDATE users SET field1 = value1, field2 = value2 WHERE username = ?", username)
        return field, nil
    case "INCREASE":
		// Handle INCREASE operation
		// Example: Increase the specified field value
		var currentValue int
		err := conn.QueryRow("SELECT "+field.Name+" FROM users WHERE username = ?", username).Scan(&currentValue)
		if err != nil {
			return field, err
		}
	
		// Ensure the increase is valid (value after increase should not overflow)
		increaseAmount, err := strconv.Atoi(field.Value)
		if err != nil {
			return field, err
		}
	
		if currentValue <= (int(^uint(0) >> 1) - increaseAmount) {
			_, err := conn.Exec("UPDATE users SET "+field.Name+" = "+field.Name+" + ? WHERE username = ?", increaseAmount, username)
			if err != nil {
				return field, err
			}
	
			// Set the new value after the increase
			field.Value = strconv.Itoa(currentValue + increaseAmount)
			return field, nil
		} else {
			return field, fmt.Errorf("Cannot increase %s. Resulting value would overflow.", field.Name)
		}
	
	case "DECREASE":
		// Handle DECREASE operation
		// Example: Decrease the specified field value
		var currentValue int
		err := conn.QueryRow("SELECT "+field.Name+" FROM users WHERE username = ?", username).Scan(&currentValue)
		if err != nil {
			return field, err
		}
	
		// Ensure the decrease is valid (value after decrease should not be negative)
		decreaseAmount, err := strconv.Atoi(field.Value)
		if err != nil {
			return field, err
		}
	
		if currentValue >= decreaseAmount {
			_, err := conn.Exec("UPDATE users SET "+field.Name+" = "+field.Name+" - ? WHERE username = ?", decreaseAmount, username)
			if err != nil {
				return field, err
			}
	
			// Set the new value after the decrease
			field.Value = strconv.Itoa(currentValue - decreaseAmount)
			return field, nil
		} else {
			return field, fmt.Errorf("Cannot decrease %s. Insufficient value.", field.Name)
		}
    default:
        return field, fmt.Errorf("Unsupported operation type: %s", field.OperationType)
    }
}






func handleGetRandomPlayer(w http.ResponseWriter, r *http.Request) {
    if r.Method != http.MethodPost {
        http.Error(w, "Invalid request method", http.StatusMethodNotAllowed)
        return
    }

    // Decode JSON from the request body
    var requestBody UserRequest
    decoder := json.NewDecoder(r.Body)
    if err := decoder.Decode(&requestBody); err != nil {
        http.Error(w, "Failed to decode request body", http.StatusBadRequest)
        return
    }

    log.Println("IOKK", requestBody.Username)

    // Verify the token
    valid, nickname, err := VerifyToken(requestBody.Username)
    if err != nil || !valid {
        log.Println("Invalid token or username:", requestBody.Username)
        http.Error(w, "Unauthorized: Invalid token or username", http.StatusUnauthorized)
        return
    }

    // Open a connection to the SQLite database
    conn, err := sql.Open("sqlite3", "example.db")
    if err != nil {
        log.Println("Error opening SQLite database:", err)
        http.Error(w, "Internal Server Error", http.StatusInternalServerError)
        return
    }
    defer conn.Close()

    userFields := requestBody.Fields

    // Select a random user whose username is different from the provided nickname and has more than 1000 coins
    var randomUser LUser
    err = conn.QueryRow("SELECT username, coins FROM users WHERE username != ? AND coins > 1000 ORDER BY RANDOM() LIMIT 1", nickname).
        Scan(&randomUser.Username, &randomUser.Coins)
    if err != nil {
        log.Println("Error scanning random user data:", err)
        http.Error(w, "Internal Server Error", http.StatusInternalServerError)
        return
    }

    // Create a new UserField for the "Coins" information
    coinsField := UserField{
        OperationType: "GET", // or "SET", "UPDATE", etc., depending on your use case
        Name:          "Coins",
        Value:         randomUser.Coins,
    }

    // Append the new Coins field to the existing userFields slice
    userFields = append(userFields, coinsField)

    // Construct the UserResponse with the updated userFields
    response := UserResponse{
        Success:  true,
        Message:  "User data retrieved successfully",
        Username: randomUser.Username,
        Fields:   userFields,
    }

    w.Header().Set("Content-Type", "application/json")
    json.NewEncoder(w).Encode(response)
}




// AddCoinsRequest represents the request payload for adding or decreasing coins
type AddCoinsRequest struct {
	Username string `json:"username"`
	Operation string `json:"operation"` // "add" or "decrease"
	Amount int `json:"amount"`
}

// Endpoint to handle adding or decreasing coins
func handleAddCoins(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Invalid request method", http.StatusMethodNotAllowed)
		return
	}

	// Extract the token from the Authorization header
	tokenString := r.Header.Get("Authorization")
	if tokenString == "" {
		http.Error(w, "Unauthorized: Missing token", http.StatusUnauthorized)
		return
	}

	// Verify the token
	valid, username, err := VerifyToken(tokenString)
	if err != nil || !valid {
		// If the token is invalid, assume it might be a username and check if it exists in the database
		validUsername, err := CheckUsernameValidity(tokenString)
		if err != nil || !validUsername {
			log.Println("Invalid token or username:", tokenString)
			http.Error(w, "Unauthorized: Invalid token or username", http.StatusUnauthorized)
			return
		}
		// Use the provided string as the username
		username = tokenString
	}

	log.Println("Username: " + username)

	// Decode the request payload
	var addCoinsRequest AddCoinsRequest
	err = json.NewDecoder(r.Body).Decode(&addCoinsRequest)
	if err != nil {
		log.Println("Error decoding request:", err)
		http.Error(w, "Bad Request", http.StatusBadRequest)
		return
	}

	// Open a connection to the SQLite database
	conn, err := sql.Open("sqlite3", "example.db")
	if err != nil {
		log.Println("Error opening SQLite database:", err)
		http.Error(w, "Internal Server Error", http.StatusInternalServerError)
		return
	}
	defer conn.Close()

	// Determine the operation (add or decrease)
	switch addCoinsRequest.Operation {
	case "add":
		_, err = conn.Exec("UPDATE users SET coins = coins + ? WHERE username = ?", addCoinsRequest.Amount, username)
	case "decrease":
		_, err = conn.Exec("UPDATE users SET coins = coins - ? WHERE username = ? AND coins >= ?", addCoinsRequest.Amount, username, addCoinsRequest.Amount)
	default:
		http.Error(w, "Invalid operation", http.StatusBadRequest)
		return
	}

	if err != nil {
		log.Println("Error updating coins:", err)
		http.Error(w, "Internal Server Error", http.StatusInternalServerError)
		return
	}

	// Respond with success message
	response := map[string]interface{}{
		"Success": true,
		"Message": "Coins updated successfully",
	}
	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(response)
}

// CheckUsernameValidity checks if the given string is a valid username in the database
func CheckUsernameValidity(username string) (bool, error) {
	conn, err := sql.Open("sqlite3", "example.db")
	if err != nil {
		return false, err
	}
	defer conn.Close()

	var count int
	err = conn.QueryRow("SELECT COUNT(*) FROM users WHERE username = ?", username).Scan(&count)
	if err != nil {
		return false, err
	}

	return count > 0, nil
}





func main() {
	// Set up logging to a file
	logFile, err := os.Create("server.log")
	if err != nil {
		log.Fatal("Error creating log file:", err)
	}
	defer logFile.Close()

	// Log server start
	log.Println("Server started on port 8080...")

	// Register handlers for login and attack endpoints
	http.HandleFunc("/verify", verifyTokenHandler)
	http.HandleFunc("/login", handleLoginRequest)
	http.HandleFunc("/attack", handleAttackRequest)
	http.HandleFunc("/data", handleGetData)
	http.HandleFunc("/add-coins", handleAddCoins)
	http.HandleFunc("/randomplayer", handleGetRandomPlayer)

	// Start the HTTP server
	log.Fatal(http.ListenAndServe(":8080", nil))
}
